import express from 'express';

import environment from './env';
import router from '../router/catalogRouter';
import cors from "cors";
import helmet from "helmet";
import * as morgan from "morgan";
import * as winston from "winston";
import * as winstonExpress from "express-winston";


class Express {
	/**
	 * Create the express object
	 */
	public express: express.Application;

	/**
	 * Initializes the express server
	 */
	constructor () {
		this.express = express();

		this.mountDotEnv();
		this.mountMiddlewares();
		this.mountRoutes();
	}

	private mountDotEnv (): void {
		this.express = environment.init(this.express);
	}

	/**
	 * Mounts all the defined middlewares
	 */
	private mountMiddlewares (): void {
		// this.express = Bootstrap.init(this.express);
            this.express.use(helmet());
            this.express.use(cors());
            this.express.use(express.json());
            this.express.use(morgan.default("combined"));
            const loggerOptions: winstonExpress.LoggerOptions = {
                transports: [new winston.transports.Console()],
                format: winston.format.combine(
                    winston.format.json(),
                    winston.format.prettyPrint(),
                    winston.format.colorize({ all: true })
                ),
            };

            if (!process.env.DEBUG) {
                loggerOptions.meta = false; // when not debugging, log requests as one-liners
            }
	}

	/**
	 * Mounts all the defined routes
	 */
	private mountRoutes (): void {
		this.express.use(`/${environment.config().apiPrefix}`, router);
	}

	/**
	 * Starts the express server
	 */
	public init (): void {
		const port: number = environment.config().port;

		// Registering Exception / Error Handlers
		// this.express.use(ExceptionHandler.logErrors);
		// this.express.use(ExceptionHandler.clientErrorHandler);
		// this.express.use(ExceptionHandler.errorHandler);
		// this.express = ExceptionHandler.notFoundHandler(this.express);

		// Start the server on the specified port
		this.express.listen(port, () => {
			return console.log('\x1b[33m%s\x1b[0m', `Server :: Running @ 'http://localhost:${port}'`);
		}).on('error', (_error) => {
			return console.log('Error: ', _error.message);
		})		
	}
}

/** Export the express module */
export default new Express();
