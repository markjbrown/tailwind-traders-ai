class OrderDao {
    constructor(cosmosClient, databaseId) {
      this.client = cosmosClient;
      this.databaseId = databaseId;
      this.collectionId = "orders";
  
      this.database = null;
      this.container = null;
    }
  
    async init() {
      const dbResponse = await this.client.databases.createIfNotExists({
        id: this.databaseId
      });
      this.database = dbResponse.database;
      const coResponse = await this.database.containers.createIfNotExists({
        id: this.collectionId
      });
      this.container = coResponse.container;
    }
  
    async createOrder(email, items) {
        //TODO: need date/time
        const order = { email: email, items: items,createdOn:Number(new Date()) };
        const { resource: doc } = await this.container.items.create(order);
        return doc;
    }
}

module.exports = OrderDao;