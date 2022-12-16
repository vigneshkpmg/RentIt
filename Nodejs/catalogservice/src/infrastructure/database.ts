
import mongoose from "mongoose";
import env from "./env"
import logger from "./logger";
export default class database{

    public static async Init(): Promise<void>{
        const mongooseUrl = env.config().mongooseUrl;
        try {
            logger.info("Connecting to mongo db");
            mongoose.set('strictQuery', true);
            await mongoose.connect(mongooseUrl);
            logger.info("Connected to mongo db");
        } catch (error) {
            logger.error("failed to connect to mongo db", error);          
        }
    }
}

export {mongoose}