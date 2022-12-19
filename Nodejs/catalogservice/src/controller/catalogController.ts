import express from "express";
import logger from "../infrastructure/provider/logger";
import catalog from "../model/catalog";

class catalogController{
    
    public async getcatalogItems(req: express.Request, res: express.Response): Promise<any>{
        try { 
            const catalogList = await catalog.find({});
            return res.status(200).send({ data: catalogList });
        }
        catch (error) {
            logger.error("Error while fetching list of catalog", error);
            return res.status(500).send("Internal server error !");
        }
    }

     public async getCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
         try {
             const id = req.params.id
             const catalogData = await catalog.findById(id);
             return res.status(200).send({ data: catalogData });
         }
         catch (error) {
             logger.error("Error while fetching catalog by id", error);
             return res.status(500).send("Internal server error !");
         }
     }
    
    public async createCatalogItem(req: express.Request, res: express.Response): Promise<any> {
        const quantity = req.body.quantity;
        const catalogList = [];
        try {
            //Todo: Need to save media and get the url.
            for (let index = 0; index < quantity; index++) {
                const catalogInstance = new catalog({
                    title: req.body.title,
                    availabilityStartDate: req.body.availabilityStartDate,
                    description: req.body.description,
                    sellerReferenceId: req.body.sellerReferenceId,
                    equipmentRentPrice: req.body.equipmentRentPrice,
                    catalogType: req.body.catalogType,
                    isOperatorAvailable: req.body.isOperatorAvailable,
                    operatorPrice: req.body.operatorPrice,
                    additionalDetails: req.body.additionalDetails,
                    mediaUrl: ""
                });
                catalogList.push(catalogInstance);
            }
            const result = await catalog.bulkSave(catalogList);
            return res.status(201).send(result);
        }
        catch (error) {
            logger.error("Error while saving catalog", error);
            return res.status(500).send("Internal server error!");
        }
    }
  
    public async updateCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
        try {
            const id = req.params.id;
            const catalogInstance = new catalog({
                    title: req.body.title,
                    availabilityStartDate: req.body.availabilityStartDate,
                    description: req.body.description,
                    sellerReferenceId: req.body.sellerReferenceId,
                    equipmentRentPrice: req.body.equipmentRentPrice,
                    catalogType: req.body.catalogType,
                    isOperatorAvailable: req.body.isOperatorAvailable,
                    operatorPrice: req.body.operatorPrice,
                    additionalDetails: req.body.additionalDetails,
                    mediaUrl: ""
            });
           const result = await catalog.findByIdAndUpdate(id, catalogInstance, {overwrite:true});
           return res.status(200).send(result);
        }
        catch (error) {
            logger.error("Error while updating catalog by id", error);
            return res.status(500).send("Internal server error !");
        }
     }
    
    public async deleteCatalogItemById(req: express.Request, res: express.Response): Promise<any>{
        try {
            const result = await catalog.findByIdAndDelete(req.params.id);
            return res.status(204).send(result);

        }
        catch (error) {
            logger.error("Error while deleting catalog by id", error);
            return res.status(500).send("Internal server error !");
        }
     }
    
     public async updateCatalogItem(req: express.Request, res: express.Response): Promise<any>{
         try {
             const id = req.params.id;
             const request = req.body;
             const result = await catalog.findByIdAndUpdate(id, request);
             return res.status(200).send(result);
         }
         catch (error) {
             logger.error("Error while patching catalog by id", error);
            return res.status(500).send("Internal server error !");
         }
    }
}

export default new catalogController();