const { v4: uuidv4 } = require("uuid");
const createTableIfNeeded = require("./dynamoCommon");

class OrderDao {
  constructor(awsClient, tableName, documentClient) {
    this.client = awsClient;
    this.tableName = tableName;
    this.documentClient = documentClient;
  }

  async init() {
    await createTableIfNeeded(this.client, this.tableName);
  }

  async createOrder(email, items) {
    let inputDoc = {};
    inputDoc.createdOn = Number(new Date());
    inputDoc.id = uuidv4();
    inputDoc.email = email;
    inputDoc.items = items;

    const params = {
      TableName: this.tableName,
      Item: inputDoc,
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

  async getPopularProducts() {
    const params = {
      FilterExpression: "createdOn > :dt",
      ExpressionAttributeValues: { ":dt": Number(new Date()) - 2592000000 },
      TableName: this.tableName,
    };

    var orderDetails = await this.documentClient.scan(params).promise();

    if (orderDetails.Items.empty) {
      console.log("No matching documents.");
      return result.Items;
    }

    var result = [];

    await orderDetails.Items.forEach((doc) => {
      let res = doc.items;

      res.forEach((item) => {
        result.push({ id: item.id, qty: item.qty });
      });
    });

    // Grouped array
    const grouped = [];

    // grouping by id and resulting with an object using Array.reduce() method
    const groupById = result.reduce((group, item) => {
      const { id } = item;
      group[id] = group[id] || [];
      group[id].push(item.qty);
      return group;
    }, {});

    // Finally calculating the sum based on the id array we have.
    Object.keys(groupById).forEach((item) => {
      groupById[item] = groupById[item].reduce((a, b) => a + b);
      grouped.push({
        id: Number(item),
        qty: groupById[item],
      });
    });

    //Sorting the grouped array by qty in descending order
    var sorted = grouped.sort((x, y) => {
      return x.qty == y.qty ? 0 : x.qty < y.qty ? 1 : -1;
    });

    //Array of top five ordered Ids
    var arrIds = [];

    //Pushing top five highest ordered items  in to array
    for (let i = 0; i <= Math.min(sorted.length - 1, 4); i++) {
      arrIds.push(sorted[i].id);
    }

    //fetching the Product details from ProductItem collection for top five ordered items

    var finalResult = [];
    for (let i = 0; i < arrIds.length; i++) {
      var params2 = {
        ExpressionAttributeValues: {
          ":s": arrIds[i],
        },
        KeyConditionExpression: "Id = :s",
        ExpressionAttributeNames: { "#c": "Name" },
        ProjectionExpression: "Id,ImageName,#c,Price,TypeId,TagId",
        TableName: "ProductItem",
      };
      var sn2 = await this.documentClient.query(params2).promise();
      finalResult.push(sn2.Items[0]);
    }

    return finalResult;
  }
}

module.exports = OrderDao;
