import express from "./infrastructure/express"
import database from "./infrastructure/database";

class StartUp{

    public static init(): void{
        database.Init();
        express.init();
    }
}

StartUp.init();