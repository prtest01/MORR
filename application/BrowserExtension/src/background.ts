import '@babel/polyfill'
import { BrowserEvent } from './Shared/SharedDeclarations'
import { ICommunicationStrategy, PostHTTPInterface } from './ApplicationInterface/';
import ListenerManager from "./ListenerManager"

enum ExtensionState {
    Disconnected,
    Ready,
    Recording
}

/**
 * The "main" class of the webextension
 */
export default class BackgroundScript {
    /**
     * ListenerManager controlled by background script
     */
    listenerManager : ListenerManager;
    /**
     * App interface to the MORR application
     */
    appInterface : ICommunicationStrategy;

    /**
     * Helper variables
     */
    private configString : string | undefined;
    private isConnected : boolean;
    private static readonly RETRYDELAYMS = 5000;
    private isRecording : boolean;
    private static readonly defaultTitle : string = "MORR Browser Extension"; //default title for the browser extension icon
    /**
     * Creates an instance of background script and initializes the listeners.
     */
    constructor() {
        this.isConnected = false;
        this.listenerManager = new ListenerManager(this.callback);
        this.appInterface = new PostHTTPInterface();
        this.appInterface.addOnStopListener((error? : boolean) => {
            if (!error)
                this.stop();
            else
                this.reset();
        });
        this.isRecording = false;
        this.setStatusIcon(ExtensionState.Disconnected);
    }

    /**
     * Start all listeners
     */
    private start = () : void => {
        if (!this.isRecording) {
            this.isRecording = true;
            this.listenerManager.startAll();
            console.log("BackgroundScript started.");
            this.setStatusIcon(ExtensionState.Recording);
        }
    }
    /**
     * Stop all listeners and wait for next start signal
     */
    private stop = () : void => {
        if (this.isRecording) {
            this.setStatusIcon(ExtensionState.Ready);
            this.isRecording = false;
            this.listenerManager.stopAll();
            this.appInterface.waitForStart()
            .then(() => this.start())
            .catch((e) => {
                this.reset();
            });
            console.log("BackgroundScript stopped.");
        }
    }

    //completely reset the connection status
    private reset = () : void => {
        if (this.isConnected) {
            this.isConnected = false;
            if (this.isRecording) {
                this.isRecording = false;
                this.listenerManager.stopAll();
            }
            setTimeout(this.run, BackgroundScript.RETRYDELAYMS);
        }
    }


    /**
     * Connect to the main application and start recording when receiving the corresponding signal from the main application.
     */
    public run = () : void => {
        this.isConnected = false;
        this.setStatusIcon(ExtensionState.Disconnected);
        this.establishConnection(true)
        .then(() => {
            this.isConnected = true;
            this.setStatusIcon(ExtensionState.Ready);
            return this.appInterface.waitForStart();
        }).then(() => this.start())
        .catch(() => {
            this.isConnected = false;
            setTimeout(this.run, BackgroundScript.RETRYDELAYMS)
        });
    }

    /**
     * Callback to hand over to the listeners
     */
    public callback = (event : BrowserEvent) : void => {
        console.log(`${BackgroundScript.timeStampString(event.timeStamp)}: ${event.type} occured in tab ${event.tabID} in window ${event.windowID}`);
        this.appInterface.sendData(event.serialize(true))
        .catch(() => {
            this.reset();
        });
    }

    /**
     * Helper Function. Generates timesstampstring for console/debug output
     * @param date the Date to represent
     * @returns string representing the date
     */
    private static timeStampString(date : Date) : string {
        return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}:${date.getMilliseconds()}`;
    }

    /**
     * Set up a connection to the main application using the appInterface.
     * @param getConfig set to true if a (new) configuration string should be requested, false otherwise
     */
    private establishConnection = (getConfig : boolean) : Promise<void> => {
        return new Promise((resolve, reject) => {
            this.appInterface.establishConnection()
            .then(() => {
                if (getConfig) {
                    return this.appInterface.requestConfig()
            } else {
                return Promise.resolve(undefined);
            }})
            .then((configString : string | undefined) => {
                if (configString)
                    this.configString = configString;
                resolve();
            })
            .catch((err : string) => {
                console.log(`Error during initialization: ${err}. Retrying in ${BackgroundScript.RETRYDELAYMS} ms.`);
                setTimeout(() => {
                    this.establishConnection(getConfig)
                    .then(() => resolve());
                }, BackgroundScript.RETRYDELAYMS);
            });
        });
    }

    /**
     * @deprecated
     * Parses configuration string and applies the configuration by setting the respective values in the web storage.
     * @param configuration The configuration string (JSON)
     * @returns true if configuration was valid, false otherwise
     */
    private parseAndApplyConfiguration(configuration : string) : boolean {
        throw new Error("Method not implemented.");
    }

    /**
     * Sets the badge in the browser tray
     * @param status the current status
     */
    private setStatusIcon(status : ExtensionState) : void {
        let label : string;
        let color : string;
        switch(status) {
            case(ExtensionState.Disconnected):
                chrome.browserAction.setIcon({path: "logos/morr_default.png"});
                chrome.browserAction.setTitle({title: `${BackgroundScript.defaultTitle}\nDisconnected`});
                label = "DC";
                color = "#333333"
                break;
            case(ExtensionState.Ready):
                chrome.browserAction.setIcon({path: "logos/morr_default.png"});
                chrome.browserAction.setTitle({title: `${BackgroundScript.defaultTitle}\nReady`});
                label = "";
                color = "#00AA00";
                break;
            case(ExtensionState.Recording):
                chrome.browserAction.setIcon({path: "logos/morr_active_recording.png"});
                chrome.browserAction.setTitle({title: `${BackgroundScript.defaultTitle}\nRecording`});
                label = "";
                color = "#AA0000";
                break;
            default:
                label = "ERR";
                color = "#000000";
        }
        chrome.browserAction.setBadgeText({text : label});
        chrome.browserAction.setBadgeBackgroundColor({color : color});
    }
}

//entry point
let main = new BackgroundScript()
main.run();