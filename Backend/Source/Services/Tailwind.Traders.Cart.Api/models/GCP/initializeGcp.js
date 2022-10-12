const config = require("../../config/config");
const CartController = require("../../routes/cartController");
const ShoppingCartDao = require("./shoppingCartDao");
const RecommededDao = require("./recommendedDao");
const OrderDao = require("./orderDao");

function initializeGcp() {
  var admin = require("firebase-admin");
  var serviceAccount = config.serviceAccount;
  var firebaseSettings = admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
  });

  const firebaseClient = firebaseSettings.firestore();
  const recommendedDao = new RecommededDao(firebaseClient);
  const shoppingCartDao = new ShoppingCartDao(
    firebaseClient,
    config.collectionId
  );
  const orderDao = new OrderDao(firebaseClient, "orders");
  const cartController = new CartController(
    shoppingCartDao,
    recommendedDao,
    orderDao
  );
  recommendedDao
    .init((err) => {
      console.error(err);
    })
    .then(() => {
      console.log(`FireBase  initialized`);
    })
    .catch((err) => {
      console.error(err);
      console.error(
        "Shutting down because there was an error setting up the database.2"
      );
      process.exit(1);
    });

  return cartController;
}

module.exports = initializeGcp;
