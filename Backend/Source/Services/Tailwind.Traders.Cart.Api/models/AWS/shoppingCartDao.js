const { v4: uuidv4 } = require("uuid");
const createTableIfNeeded = require("./dynamoCommon");

class ShoppingCartDao {
  constructor(awsClient, tableName, documentClient) {
    this.client = awsClient;
    this.tableName = tableName;
    this.documentClient = documentClient;
  }

  async init() {
    await createTableIfNeeded(this.client, this.tableName);
  }

  async find(email) {
    const params = {
      FilterExpression: "detailProduct.email = :email",
      ExpressionAttributeValues: { ":email": email },
      TableName: this.tableName,
    };

    var snapshot = await this.documentClient.scan(params).promise();

    if (snapshot.Items.empty) {
      console.log("No matching documents.");
      return result.Items;
    }

    return snapshot.Items.map((i) => ({
      id: i.detailProduct.id,
      name: i.detailProduct.name,
      price: i.detailProduct.price,
      imageUrl: i.detailProduct.imageUrl,
      email: i.detailProduct.email,
      qty: i.qty,
      _cdbid: i.id,
    }));
  }

  async addItem(item) {
    item.id = uuidv4();
    const params = {
      TableName: this.tableName,
      Item: item,
    };

    var result = await this.documentClient
      .put(params)
      .promise()
      .then(
        (data) => {
          return params;
        },
        (error) => {
          return error;
        }
      );
    return result.Item;
  }

  async updateQuantity(id, newqty) {
    const params = {
      TableName: this.tableName,
      Key: {
        id: id,
      },
      UpdateExpression: "set qty = :qty",
      ExpressionAttributeValues: {
        ":qty": newqty,
      },
      ReturnValues: "ALL_NEW",
    };

    var result = await this.documentClient
      .update(params)
      .promise()
      .then(
        (data) => {
          return data;
        },
        (error) => {
          return error;
        }
      );
    return result;
  }

  async deleteItem(id) {
    let params = {
      TableName: this.tableName,
      Key: {
        id: id,
      },
      ReturnValues: "ALL_OLD",
    };
    var result = await this.documentClient
      .delete(params)
      .promise()
      .then(
        (data) => {
          return data;
        },
        (error) => {
          return error;
        }
      );
    return result;
  }
}

module.exports = ShoppingCartDao;
