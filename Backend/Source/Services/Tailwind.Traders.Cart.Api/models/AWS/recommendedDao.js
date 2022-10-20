const createTableIfNeeded = require("./dynamoCommon");

class RecommendedDao {
  constructor(awsClient, tableName, documentClient) {
    this.client = awsClient;
    this.tableName = tableName;
    this.documentClient = documentClient;
  }

  async init() {
    await createTableIfNeeded(this.client, this.tableName);
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
