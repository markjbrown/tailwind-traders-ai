const { setAuthorizationTokenHeaderUsingMasterKey } = require("@azure/cosmos");

class OrderDao {
  constructor(cosmosClient, databaseId) {
    this.client = cosmosClient;
    this.databaseId = databaseId;
    this.collectionId = "orders";
    this.collectionIdProducts = "ProductItem";
    this.database = null;
    this.container = null;
    this.containerProducts = null;
  }

  async init() {
    const dbResponse = await this.client.databases.createIfNotExists({
      id: this.databaseId,
    });
    this.database = dbResponse.database;
    const coResponse = await this.database.containers.createIfNotExists({
      id: this.collectionId,
    });
    this.container = coResponse.container;

    const coResponseProducts = await this.database.containers.createIfNotExists(
      {
        id: this.collectionIdProducts,
      }
    );
    this.container = coResponse.container;
    this.containerProducts = coResponseProducts.container;
  }

  async createOrder(email, items) {
    const order = { email: email, items: items, createdOn: Number(new Date()) };
    const { resource: doc } = await this.container.items.create(order);
    return doc;
  }

  async getPopularProducts() {
    var querySpec = {
      query: "SELECT * FROM r WHERE  r.createdOn > @createdOn",
      parameters: [
        {
          name: "@createdOn",
          value: Number(new Date()) - 2592000000,
        },
      ],
    };
    if (!this.container) {
      throw new Error("Collection is not initialized.");
    }
    const { resources: orders } = await this.container.items
      .query(querySpec)
      .fetchAll();

    var result = [];
    for (let i = 0; i < orders.length; i++) {
      await orders[i].items.map((i) => {
        result.push({ id: i.id, qty: i.qty });
      });
    }

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

    // Array of top five ordered Ids
    var arrIds = "";

    // Pushing top five highest ordered items  in to array
    for (let i = 0; i <= Math.min(sorted.length - 1, 4); i++) {
      arrIds =
        i == 0
          ? arrIds + String(sorted[i].id)
          : (arrIds = arrIds + "," + String(sorted[i].id));
    }

    querySpec = {
      query: "SELECT * FROM r WHERE  r.Id in (" + arrIds + ")",
    };

    console.log(querySpec);
    if (!this.containerProducts) {
      throw new Error("Collection is not initialized.");
    }

    // fetching the Product details from ProductItem collection for top five ordered items
    const { resources: products } = await this.containerProducts.items
      .query(querySpec)
      .fetchAll();

    return products.map((i) => ({
      id: i.Id,
      name: i.Name,
      imageName: i.ImageName,
      typeId: i.TypeId,
      TagId: i.TagId,
      brandId: i.BrandId,
    }));
  }
}

module.exports = OrderDao;
