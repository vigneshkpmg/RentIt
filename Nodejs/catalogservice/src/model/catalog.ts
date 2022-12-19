import { Schema } from "mongoose"
import {mongoose} from "../infrastructure/provider/database"


const catalogSchema = new mongoose.Schema<IEquipmentCatalog>({
    additionalDetails: Schema.Types.Mixed,
    sellerReferenceId: { type: String, require: [true, "Required"] },
    title: { type: String, require: [true, "Required"] },
    mediaUrl: { type: String, require: [true, "Required"] },
    availabilityStartDate: { type: Date, require: [true, "Required"] },
    equipmentRentPrice: { type: Number, require: [true, "Required"] },
    isOperatorAvailable: { type: Boolean, default: false },
    operatorPrice: { type: Number },
    description: { type: String, required: [true, "Required"] },
    catalogType: {type:String, required:[true,"Required"]}
}, {strict:false})

interface Icatalog{
    sellerReferenceId: string,
    catalogType: string,
    title: string,
    description: string,
    availabilityStartDate: Date,
    mediaUrl: string,
    additionalDetails:IadditionalDetails
}

interface IEquipmentCatalog extends Icatalog  {
    equipmentRentPrice: number,
    isOperatorAvailable: boolean,
    operatorPrice:Number
}

interface IadditionalDetails{
    [name:string]:string
}

const catalog = mongoose.model<IEquipmentCatalog>('catalog', catalogSchema);

export default catalog;