import express from "express";

class catalogController{
    
    public async catalogList(req: express.Request, res:express.Response): Promise<any>{
        return res.status(200).send("All good");
    }
}

export default new catalogController();