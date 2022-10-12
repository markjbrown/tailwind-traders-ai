const config = require("../../config/config");
const CartController = require("../../routes/cartController");
const ShoppingCartDao = require("./shoppingCartDao");
const RecommededDao = require("./recommendedDao");
const OrderDao = require("./orderDao");

function initializeAws() {
  const AWS = require("aws-sdk");
  AWS.config.update({
    region: process.env.AWS_DEFAULT_REGION,
    accessKeyId: process.env.AWS_ACCESS_KEY_ID,
    secretAccessKey: process.env.AWS_SECRET_ACCESS_KEY,
  });

  const client = new AWS.DynamoDB();
  const documentClient = new AWS.DynamoDB.DocumentClient();
  const recommendedDao = new RecommededDao(
    client,
    config.tableName,
    documentClient
  );
  const shoppingCartDao = new ShoppingCartDao(
    client,
    config.tableName,
    documentClient
  );
  const orderDao = new OrderDao(client, "orders", documentClient);
  const cartController = new CartController(
    shoppingCartDao,
    recommendedDao,
    orderDao
  );
  recommendedDao.init();
  shoppingCartDao.init();
  orderDao.init();

  return cartController;
}

module.exports = initializeAws;
