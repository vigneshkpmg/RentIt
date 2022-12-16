import { Schema } from "mongoose"
import {mongoose} from "../infrastructure/database"


const catalogSchema = new mongoose.Schema<Icatalog>({
    additionalDetails: Schema.Types.Mixed,
    sellerReferenceId: { type: String, require: [true, "Required"] },
    title: { type: String, require: [true, "Required"] },
    quantity: { type: Number, require: [true, "Required"] },
    mediaUrl: { type: String, require: [true, "Required"] },
    availabilityStartDate: { type: Date, require: [true, "Required"] },
    price: { type: Number, require: [true, "Required"] }   
}, {strict:false})

interface Icatalog{
    sellerReferenceId:string,
    title: string,
    quantity: number,
    mediaUrl: string,
    availabilityStartDate: Date,
    availabilityEndDate: Date,
    price: number,
    additionalDetails:IadditionalDetails
}

interface IadditionalDetails{
    [name:string]:string
}

const catalog = mongoose.model<Icatalog>('catalog', catalogSchema);

export default catalog;