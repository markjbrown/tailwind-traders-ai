require("dotenv").config();

console.log(`CLOUD_PLATFORM ${process.env.CLOUD_PLATFORM}`);

const config = {};

if (process.env.CLOUD_PLATFORM === "AZURE") {
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

if (process.env.CLOUD_PLATFORM === "GCP") {
  // try {
  //   var serviceAccount = require("../serviceKey.json");
  // } catch (err) {
  //   console.error("Error While reading serviceKey.json file");
  //   process.exit(1);
  // }

  // config.serviceAccount = serviceAccount;
  config.collectionId = "Products";
  config.databaseURL = process.env.DATABASEURL;
}

if (process.env.CLOUD_PLATFORM === "AWS") {
  config.serviceAccount = serviceAccount;
  config.cartProductsTable = process.env.AWS_CART_PRODUCTS_TABLE;
  config.cartRecommendationsTable = process.env.AWS_CART_RECOMMENDATIONS_TABLE;
  config.cartOrdersTable = process.env.AWS_CART_ORDERS_TABLE;
}

module.exports = config;
