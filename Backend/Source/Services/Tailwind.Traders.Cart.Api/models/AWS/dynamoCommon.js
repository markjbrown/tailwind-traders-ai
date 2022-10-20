async function createTableIfNeeded(client, tableName) {
  try {
    var params = {
      TableName: tableName,
      KeySchema: [{ AttributeName: "id", KeyType: "HASH" }],
      AttributeDefinitions: [{ AttributeName: "id", AttributeType: "S" }],
      ProvisionedThroughput: {
        ReadCapacityUnits: 5,
        WriteCapacityUnits: 5,
      },
    };

    const result = await client.listTables({}).promise();
    if (result.TableNames.filter((n) => n == tableName).length > 0) {
      console.log(`Found table ${tableName}`);
    } else {
      console.log(`Creating table ${tableName}`);
      await client.createTable(params).promise();
    }
  } catch (error) {
    console.log("Unable to Initialize DynamoDB!!!!", error);
    process.exit(1);
  }
}

module.exports = createTableIfNeeded