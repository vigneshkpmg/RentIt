/**
 * Define all your API web-routes
 */

import { Router } from 'express';

import catalog from '../controller/catalogController'

const router = Router();

router.get('/', catalog.catalogList);
export default router;
