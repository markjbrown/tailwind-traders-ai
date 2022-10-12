const config = require("./config/config");
const authConfig = require("./config/authConfig");
const CartController = require("./routes/cartController");
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
console.log("auth Config UseB2C", authConfig.UseB2C);

if (JSON.parse(authConfig.UseB2C)) {
  const options = {
    identityMetadata: authConfig.identityMetadata,
    issuer: authConfig.issuer,
    clientID: authConfig.clientID,
    policyName: authConfig.policyName,
    isB2C: true,
    validateIssuer: true,
    loggingLevel: "info",
    passReqToCallback: false,
  };

  const bearerStrategy = new BearerStrategy(options, function (token, done) {
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

let cartController;
if (process.env.CLOUD_PLATFORM == "AZURE") {
    const initializeAzure = require("./models/AZURE/initializeAzure");
  cartController = initializeAzure();
}
if (process.env.CLOUD_PLATFORM == "GCP") {
  const initializeGcp = require("./models/GCP/initializeGcp");
  cartController = initializeGcp();
}
if (process.env.CLOUD_PLATFORM == "AWS") {
  const initializeAws = require("./models/AWS/initializeAws");
  cartController = initializeAws();
}

app.get("/liveness", (req, res, next) => {
  res.status(200);
  res.send("Healthy");
  return;
});

app.get("/shoppingcart", (req, res, next) =>
  cartController.getProductsByUser(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart", (req, res, next) =>
  cartController.addProduct(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart/product", (req, res, next) =>
  cartController.updateProductQuantity(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.delete("/shoppingcart/product", (req, res, next) =>
  cartController.deleteItem(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.get("/shoppingcart/relatedproducts", (req, res, next) =>
  cartController.getRelatedProducts(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);
app.post("/shoppingcart/checkout", (req, res, next) =>
  cartController.checkout(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.post("/shoppingcart/checkout", (req, res, next) =>
  cartController.checkout(req, res).catch((e) => {
    console.log(e);
    next(e);
  })
);

app.get("/popularProducts", (req, res, next) =>
  cartController.getPopulerProducts(req, res).catch((e) => {
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
    details: err,
  });
});

module.exports = app;
