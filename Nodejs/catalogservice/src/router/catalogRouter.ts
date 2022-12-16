/**
 * Define all your API web-routes
 */

import { Router } from 'express';

import catalog from '../controller/catalogController'

const router = Router();

router.get('/', catalog.getcatalogItems);
router.get('/:id', catalog.getCatalogItemById);
router.post('/', catalog.createCatalogItem);
router.put('/:id', catalog.updateCatalogItemById);
router.delete('/:id', catalog.deleteCatalogItemById);
router.patch('/:id', catalog.updateCatalogItem);
export default router;
