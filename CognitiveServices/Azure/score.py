import os
import logging
import json
import numpy as np
import pandas as pd
import turicreate
from azureml.core.model import Model

def init():
    """
    This function is called when the container is initialized/started, typically after create/update of the deployment.
    You can write the logic here to perform init operations like caching the model in memory
    """
    global model
    global prod_item_df
    
    folder_name = 'tailwind_reco_model'
    model_path = os.path.join(os.getenv("AZUREML_MODEL_DIR"), folder_name)
    logging.info(f'Model path: {model_path}')
    model = turicreate.load_model(model_path)
    
    file_path = os.path.join(os.getenv("AZUREML_MODEL_DIR"), folder_name, 'ProductItems.csv')
    prod_item_df = pd.read_csv(file_path)
    logging.info('prod_item_df size: {}'.format(len(prod_item_df)))
    
    logging.info("Init complete")

def run(raw_data):
    """
    This function is called for every invocation of the endpoint to perform the actual scoring/prediction.
    In the example we extract the data from the json input and call the scikit-learn model's predict()
    method and return the result back
    """
    logging.info("Recommender model: request received")
    data = json.loads(raw_data)["data"]
    user_id = data["user_id"]
    product_type = data["product_type"]
    logging.info(f'User id: {user_id}')
    logging.info(f'Product type: {product_type}')
    
    if (int(product_type) != -1) and (int(product_type) not in prod_item_df.TypeId.values):
        logging.info(f'Invalid product type: {product_type}')
        logging.info('Returning empty list')
        return {"recommended_products": []}
    
    if int(product_type) != -1:
        eligible_prod_ids = prod_item_df[prod_item_df.TypeId == int(product_type)].Id.values.tolist()
        factorization_recomm = model.recommend(users=[user_id], items=eligible_prod_ids, k=10)
    else:
        factorization_recomm = model.recommend(users=[user_id], k=10)
    
    recomm_df = factorization_recomm.to_dataframe()
    recco = recomm_df.ProductId.values.tolist()
    logging.info(f'Final recommendations: {recco}')
    
    #filterd_recco = [x for x in recco if x in eligible_prod_ids]
    #logging.info(f'Final recommendations: {filterd_recco}')
    
    logging.info("Request processed")
    return {
        "recommended_products": recco
    }
