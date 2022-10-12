class OrderDao {
  constructor(serviceAccount, collectionId) {
    this.Firebaseclient = serviceAccount;
    this.collectionId = collectionId;
    this.dataRef = this.Firebaseclient.collection(collectionId);
    this.dataRefProduct = this.Firebaseclient.collection("ProductItem");
  }

  async init() {}

  async createOrder(email, items) {
    try {
      const order = {
        email: email,
        items: items,
        createdOn: Number(new Date()),
      };
      let docRef = await this.dataRef.add(order);
      if (docRef.id) {
        var doc = await this.dataRef.doc(docRef.id).get();
        let result = doc.data();
        result.id = doc.id;
        return result;
      } else {
        return { message: "Record not inserted" };
      }
    } catch (e) {
      throw new Error(`Firestore DB error ${e.message} `);
    }
  }

  async getPopularProducts() {
    const snapshot = await this.dataRef
      .where("createdOn", ">", Number(new Date()) - 2592000000)
      .get();
    if (snapshot._size == 0) {
      console.log("No matching documents.");
      return [];
    }
    var result = [];

    await snapshot.forEach((doc) => {
      let res = doc.data().items;
      res.forEach((item) => {
        result.push({ id: item.id, qty: item.qty });
      });
    });

    // Grouped array
    const grouped = [];

    // grouping by id and resulting with an object using Array.reduce() method
    const groupById = result.reduce((group, item) => {
      const { id } = item;
      group[id] = group[id] ?? [];
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
    const snapshot2 = await this.dataRefProduct.where("Id", "in", arrIds).get();
    var finalResult = [];
    snapshot2.forEach((doc) => {
      let res = doc.data();

      finalResult.push({
        id: res.Id,
        price: res.Price,
        name: res.Name,
        imageName: res.ImageName,
        tag: res.Tag,
        tagId: res.TagId,
        typeId: res.TypeId,
      });
    });

    return finalResult;
  }
}

module.exports = OrderDao;
