class RecommendedDao {
  constructor(awsClient, tableName, documentClient) {
    this.client = awsClient;
    this.tableName = "recommendations";
    this.documentClient = documentClient;
  }

  async init() {
    try {
      var params = {
        TableName: this.tableName,
        KeySchema: [{ AttributeName: "id", KeyType: "HASH" }],
        AttributeDefinitions: [{ AttributeName: "id", AttributeType: "S" }],
        ProvisionedThroughput: {
          ReadCapacityUnits: 5,
          WriteCapacityUnits: 5,
        },
      };

      const tablePromise = await this.client
        .listTables({})
        .promise()
        .then((data) => {
          const exists =
            data.TableNames.filter((name) => {
              return name === this.tableName;
            }).length > 0;
          if (exists) {
            return Promise.resolve();
          } else {
            return this.client.createTable(params).promise();
          }
        });
      return tablePromise;
    } catch (error) {
      console.log("Unable to Initialize DynamoDB!!!!");
      process.exit(1);
    }
  }

  async findRelated(typeid, email) {
    let typeidToNumber = parseInt(typeid);

    const params = {
      FilterExpression: "email = :email AND typeid=:typeid",
      ExpressionAttributeValues: { ":email": email, ":typeid": typeidToNumber },
      TableName: this.tableName,
    };

    var snapshot = await this.documentClient.scan(params).promise();

    if (snapshot.Items.empty) {
      console.log("No matching documents.");
      return result.Items;
    }

    return snapshot.Items.map((i) => ({
      email: i.email,
      typeid: i.typeid,
      recommendations: i.recommendations,
    }));
  }
}

module.exports = RecommendedDao;
