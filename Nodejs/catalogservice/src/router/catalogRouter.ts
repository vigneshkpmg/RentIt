/**
 * Define all your API web-routes
 */

import express, { Router } from 'express';

import catalog from '../controller/catalogController';
import validator from '../middleware/validator';


const router = Router();

router.get('/', catalog.getcatalogItems);
router.get('/:id', catalog.getCatalogItemById);
router.post('/', validator.GetValidationRules(), validator.validate, catalog.createCatalogItem);
router.put('/:id', catalog.updateCatalogItemById);
router.delete('/:id', catalog.deleteCatalogItemById);
router.patch('/:id', catalog.updateCatalogItem);
export default router;
