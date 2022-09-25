const { v4: uuidv4 } = require('uuid');
class OrderDao {
    constructor(awsClient, tableName, documentClient) {
        this.client = awsClient;
        this.tableName = tableName;
        this.documentClient = documentClient;
      }
    
  
      async init() {
        try {
          var params = {
            TableName: this.tableName,
            KeySchema: [
              { AttributeName: "id", KeyType: "HASH" },
            ],
            AttributeDefinitions: [
              { AttributeName: "id", AttributeType: "S" },
            ],
    
            ProvisionedThroughput: {
              ReadCapacityUnits: 5,
              WriteCapacityUnits: 5
            }
    
          };
    
          const tablePromise = await this.client.listTables({})
            .promise()
            .then((data) => {
              const exists = data.TableNames
                .filter(name => {
                  return name === this.tableName;
                })
                .length > 0;
              if (exists) {
                return Promise.resolve();
              }
              else {
                return this.client.createTable(params).promise();
              }
            },
            );
          return tablePromise
        }
    
        catch (error) {
          console.log(error)
          console.log("Unable to Initialize DynamoDB!!!!")
          process.exit(1)
        }
      }
  
    async createOrder(email,items) {
        let inputDoc={}
        inputDoc.createdOn=Number(new Date())
        inputDoc.id=uuidv4();
        inputDoc.email=email
        inputDoc.items=items

          const params = {
          TableName: this.tableName,
          Item: inputDoc,
        };
    
        var result = await this.documentClient.put(params)
          .promise()
          .then((data) => {
            return params
          },
            (error) => {
              return error
            })
        return result.Item
      }
}

module.exports = OrderDao;