import express from "./infrastructure/express"

class StartUp{

    public static init(): void{
        express.init();
    }
}

StartUp.init();