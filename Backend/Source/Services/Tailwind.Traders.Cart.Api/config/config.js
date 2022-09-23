require("dotenv").config();

const config = {};

if(process.env.CLOUD_PLATFORM=="AZURE")
{
  config.cartDaoFile="./models/shoppingCartDao_azure";
  config.recommendationDaoFile ="./models/recommendedDao_azure"
config.host = process.env.HOST || "https://localhost:8081";
config.authKey = process.env.AUTHKEY;
config.databaseId = "ShoppingCart";
config.containerId = "Products";
config.applicationInsightsIK = process.env.APPLICATIONINSIGHTSIK;

if (config.host.includes("https://localhost:")) {
  console.log("Local environment detected");
  console.log(
    "WARNING: Disabled checking of self-signed certs. Do not have this code in production."
  );
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
  console.log(
    `Go to http://localhost:${process.env.PORT || "3000"} to try the sample.`
  );
}

}

if(process.env.CLOUD_PLATFORM=="GCP")
{
  
  try{
    var serviceAccount = require("../serviceKey.json");
  }
  catch{
    console.error("Error While reading serviceKey.json file")
    process.exit(1)
  }

config.cartDaoFile="./models/shoppingCartDao_gcp";
config.recommendationDaoFile ="./models/recommendedDao_gcp"
config.serviceAccount=serviceAccount;
config.collectionId = "Products";
config.databaseURL=process.env.DATABASEURL

}

if(process.env.CLOUD_PLATFORM=="AWS")
{
  
  var serviceAccount = require("../serviceKey.json");
config.cartDaoFile="./models/shoppingCartDao_aws";
config.recommendationDaoFile ="./models/recommendedDao_aws"
config.serviceAccount=serviceAccount;
config.tableName = "Products";
config.databaseURL=process.env.DATABASEURL

}







module.exports = config;
