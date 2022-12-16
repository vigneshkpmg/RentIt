import express from "express";
import logger from "../infrastructure/logger";

class catalogController{
    
    public async getcatalogItems(req: express.Request, res: express.Response): Promise<any>{
        logger.info("from default route");
        return res.status(200).send("All good");
    }

     public async getCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
        logger.info("from default route");
        return res.status(200).send("All good");
     }
    
    public async createCatalogItem(req: express.Request, res: express.Response): Promise<any> {
        logger.info("from default route");
        return res.status(200).send("All good");
    }
  
    public async updateCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
        logger.info("from default route");
        return res.status(200).send("All good");
     }
    
    public async deleteCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
        logger.info("from default route");
        return res.status(200).send("All good");
     }
    
     public async updateCatalogItem(req: express.Request, res: express.Response): Promise<any>{
        logger.info("from default route");
        return res.status(200).send("All good");
    }
}

export default new catalogController();