const CosmosClient = require("@azure/cosmos").CosmosClient;
const ConnectionPolicy = require("@azure/cosmos").ConnectionPolicy;
const config = require("./config/config");
const authConfig = require("./config/authConfig");
const CartController = require("./routes/cartController");
const ShoppingCartDao = require(config.cartDaoFile);
const RecommededDao = require(config.recommendationDaoFile);
const OrderDao = require(config.orderDaoFile);
const ensureAuthenticated = require("./middlewares/authorization");
const ensureB2cAuthenticated = require("./middlewares/authorizationB2c");
const setHeaders = require("./middlewares/headers");
const handlerHealthCheck = require("./middlewares/handlerHealthCheck");
const cors = require("cors");
const express = require("express");
const passport = require("passport");
const BearerStrategy = require("passport-azure-ad").BearerStrategy;
const cookieParser = require("cookie-parser");
const logger = require("morgan");
const bodyParser = require("body-parser");
const indexRouter = require("./routes/index");
const appInsights = require("applicationinsights");

var recommendedDao
var cartController

if (config.applicationInsightsIK) {
  appInsights.setup(config.applicationInsightsIK);
  appInsights.start();
}

const app = express();
app.use(logger("dev"));
app.use(bodyParser.json());
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cors());
app.use(cookieParser());
app.use(handlerHealthCheck);
console.log("auth Config", authConfig.UseB2C)

if (JSON.parse(authConfig.UseB2C)) {
  const options = {
    identityMetadata: authConfig.identityMetadata,
    issuer: authConfig.issuer,
    clientID: authConfig.clientID,
    policyName: authConfig.policyName,
    isB2C: true,
    validateIssuer: true,
    loggingLevel: "info",
    passReqToCallback: false
  };

  const bearerStrategy = new BearerStrategy(options, function(token, done) {
    done(null, {}, token);
  });

  app.use("/", indexRouter);

  app.use(passport.initialize());
  passport.use(bearerStrategy);

  app.use(ensureB2cAuthenticated());
} else {
  app.use(ensureAuthenticated);
}

app.use(setHeaders);


if (process.env.CLOUD_PLATFORM == "AZURE") {

  console.log(`Cosmos to use is ${config.host}`);
  console.log(`Cosmos to use is ${config.authKey}`);
  const cosmosClientOptions = {
    endpoint: config.host,
    key: config.authKey
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
  recommendedDao = new RecommededDao(cosmosClient, config.databaseId);
  const orderDao = new OrderDao(cosmosClient,
    config.databaseId,
  )

  cartController = new CartController(shoppingCartDao, recommendedDao, orderDao);

  console.log("Begin initialization of cosmosdb " + config.host);

  shoppingCartDao
    .init(err => {
      console.error(err);
    })
    .then(() => {
      console.log("HOST:", config.host)
      console.log(`cosmosdb ${config.host} initializated`);
    })
    .catch(err => {
      console.error(err);
      console.error(
        "Shutting down because there was an error setting up the database.1"
      );
      process.exit(1);
    });

  recommendedDao
    .init(err => {
      console.error(err);
    })
    .then(() => {
      console.log(`cosmosdb ${config.host} recommendations initializated`);
    })
    .catch(err => {
      console.error(err);
      console.error(
        "Shutting down because there was an error setting up the database.2"
      );
      process.exit(1);
    });

  orderDao
    .init(err => {
      console.error(err);
    })
    .then(() => {
      console.log(`cosmosdb ${config.host} orders initializated`);
    })
    .catch(err => {
      console.error(err);
      console.error(
        "Shutting down because there was an error setting up the database."
      );
      process.exit(1);
    });
}

if (process.env.CLOUD_PLATFORM == "GCP") {

  var admin = require("firebase-admin");
  var serviceAccount = config.serviceAccount;
  var firebaseSettings = admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),

  });

  firebaseClient = firebaseSettings.firestore();
  const recommendedDao = new RecommededDao(firebaseClient);
  shoppingCartDao = new ShoppingCartDao(firebaseClient, config.collectionId);
  const orderDao = new OrderDao(firebaseClient, 'orders');
  cartController = new CartController(shoppingCartDao, recommendedDao, orderDao);
  recommendedDao
    .init(err => {
      console.error(err);
    })
    .then(() => {
      console.log(`FireBase  initialized`);
    })
    .catch(err => {
      console.error(err);
      console.error(
        "Shutting down because there was an error setting up the database.2"
      );
      process.exit(1);
    });
}

if (process.env.CLOUD_PLATFORM == "AWS") {

  const AWS = require('aws-sdk');
  AWS.config.update({
    region: process.env.AWS_DEFAULT_REGION,
    accessKeyId: process.env.AWS_ACCESS_KEY_ID,
    secretAccessKey: process.env.AWS_SECRET_ACCESS_KEY,
  });

  var client = new AWS.DynamoDB()
  const documentClient = new AWS.DynamoDB.DocumentClient();
  const recommendedDao = new RecommededDao(client, config.tableName, documentClient);
  shoppingCartDao = new ShoppingCartDao(client, config.tableName, documentClient);
  const orderDao = new OrderDao(client, 'orders', documentClient);
  cartController = new CartController(shoppingCartDao, recommendedDao, orderDao);
  recommendedDao.init()
  shoppingCartDao.init()
  orderDao.init()
}

app.get("/liveness", (req, res, next) => {
  res.status(200);
  res.send("Healthy");
  return;
});

app.get("/shoppingcart", (req, res, next) =>
  cartController.getProductsByUser(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart", (req, res, next) =>
  cartController.addProduct(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart/product", (req, res, next) =>
  cartController.updateProductQuantity(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

app.delete("/shoppingcart/product", (req, res, next) =>
  cartController.deleteItem(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

app.get("/shoppingcart/relatedproducts", (req, res, next) =>
  cartController.getRelatedProducts(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart/checkout", (req, res, next) =>
  cartController.checkout(req, res).catch(e => {
    console.log(e);
    next(e);
  })
);


// catch 404 and forward to error handler
app.use(function (req, res, next) {
  const err = new Error("Not Found");
  err.status = 404;
  next(err);
});

// error handler
app.use(function (err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get("env") === "development" ? err : {};

  // render the error page
  res.status(err.status || 500);
  return res.send({
    error: "Error",
    details: err
  });
});

module.exports = app;
