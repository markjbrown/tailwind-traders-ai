const config = require("../../config/config");
const CosmosClient = require("@azure/cosmos").CosmosClient;
const ConnectionPolicy = require("@azure/cosmos").ConnectionPolicy;
const CartController = require("../../routes/cartController");
const ShoppingCartDao = require("./shoppingCartDao");
const RecommededDao = require("./recommendedDao");
const OrderDao = require("./orderDao");

function initializeAzure() {
  console.log(`Cosmos to use is ${config.host}`);
  const cosmosClientOptions = {
    endpoint: config.host,
    key: config.authKey,
  };

  const locations = process.env.LOCATIONS;
  if (locations) {
    console.log(`Preferred locations are set to: '${locations}'`);
    const connectionPolicy = new ConnectionPolicy();
    connectionPolicy.preferredLocations = locations.split(",");
    cosmosClientOptions.connectionPolicy = connectionPolicy;
  }

  const disableSSL =
    (process.env.DISABLE_SSL || "").toString().toLowerCase() === "true";
  if (disableSSL) {
    console.log(
      "Disabling SSL verification! Caution *NEVER* use this in production!"
    );
    if (cosmosClientOptions.connectionPolicy == undefined) {
      cosmosClientOptions.connectionPolicy = new ConnectionPolicy();
    }
  }

  const cosmosClient = new CosmosClient(cosmosClientOptions);

  const shoppingCartDao = new ShoppingCartDao(
    cosmosClient,
    config.databaseId,
    config.containerId
  );
  const recommendedDao = new RecommededDao(cosmosClient, config.databaseId);
  const orderDao = new OrderDao(cosmosClient, config.databaseId);

  const cartController = new CartController(
    shoppingCartDao,
    recommendedDao,
    orderDao
  );

  console.log("Begin initialization of cosmosdb " + config.host);

  shoppingCartDao
    .init((err) => {
      console.error(err);
    })
    .then(() => {
      console.log("HOST:", config.host);
      console.log(
        `cosmosdb ${config.host} ${config.containerId} initializated`
      );
    })
    .catch((err) => {
      console.error(err);
      console.error(
        `Shutting down because there was an error initializing the ${config.containerId} collection`
      );
      process.exit(1);
    });

  recommendedDao
    .init((err) => {
      console.error(err);
    })
    .then(() => {
      console.log(`cosmosdb ${config.host} recommendations initializated`);
    })
    .catch((err) => {
      console.error(err);
      console.error(
        `Shutting down because there was an error initializing the recommendations collection`
      );
      process.exit(1);
    });

  orderDao
    .init((err) => {
      console.error(err);
    })
    .then(() => {
      console.log(`cosmosdb ${config.host} orders initializated`);
    })
    .catch((err) => {
      console.error(err);
      console.error(
        `Shutting down because there was an error initializing the orders collection`
      );
      process.exit(1);
    });

  return cartController;
}

module.exports = initializeAzure;
