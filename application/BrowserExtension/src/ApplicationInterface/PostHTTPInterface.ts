import ICommunicationStrategy from './IApplicationInterface';
/**
 * Application Interface using the HTTP-POST. Expects a HTTPListener on the main application side.
 */
export default class PostHTTPInterface implements ICommunicationStrategy {
    /**
     * URL of the HTTPListener attached to the main application.
     */
    private listenerURL : string;
    constructor(url: string) {
        this.listenerURL = url;
    }
    /**
     * Establishes connection to the MORR main application
     * @returns A Promise which will be resolved when the connection is estab-
lished successfully or rejected upon connection failure or unexpected response
     */
    public establishConnection(): Promise<void> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "connect"}),
            }).then(
                response => response.json()
            ).then((response) => {
                if (response.response == "MORR") {
                    console.log("Connection established");
                    resolve();
                } else {
                    throw("Unexpected Listener");
                }
            }).catch((e) => {
                console.log(`POSTHTTPInterface error (est): ${e}`);
                reject(e);
            });
        });
    }

    /**
     * Asynchronously request a configuration string from the MORR application.
     * @returns A Promise which will be resolved and filled with the configuration string
     * as soon as the configuration string is received or rejected upon connection failure
     * or unexpected response.
     */
    public requestConfig(): Promise<string> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "config"}),

            }).then(
                response => response.json()
            ).then((response) => {
                if (response.response == "MORR" && response.config) {
                    console.log("Got config");
                    resolve(<string>response.config);
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (conf): ${e}`);
                reject(e);
            })
        });
    }

    /**
     * Asynchronously await a start signal from the MORR application.
     * @returns A Promise which will be resolved when the start signal is received
     * or rejected upon connection failure or timeout.
     */
    public waitForStart(): Promise<void> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "start"}),
            }).then(
                response => response.json()
            ).then((response) => {
                if (response.response == "MORR") {
                    console.log("Got start signal");
                    resolve();
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (waitstart): ${e}`);
                reject(e);
            })
        });
    }

    /**
     * Asynchronously send data to the MORR application.
     * @param data the JSON-data to be sent.
     * @returns A Promise which will be resolved when the data is sent successfully
     * or rejected upon connection failure or unexpected response.
     */
    public sendData(data: string): Promise<void> {
        return new Promise ((resolve, reject) => {
            fetch(this.listenerURL, {
                method : "POST",
                body: JSON.stringify({request : "sendData", data : data}),
            }).then(
                response => response.json()
            ).then((response) => {
                if (response.response == "MORR") {
                    resolve();
                } else {
                    throw("Unexpected answer");
                }
            }).catch((e) => {
                console.error(`POSTHTTPInterface error (send): ${e}`);
                reject(e);
            })
        });
    }
}
