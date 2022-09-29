class OrderDao {
  constructor(serviceAccount, collectionId) {
    this.Firebaseclient = serviceAccount;
    this.collectionId = collectionId;
    this.dataRef = this.Firebaseclient.collection(collectionId)
  }

  async init() {
  }

  async createOrder(email, items) {
    //TODO: need date/time

    try {
      const order = { email: email, items: items, createdOn: Number(new Date()) };
      let docRef = await this.dataRef.add(order);
      if (docRef.id) {
        var doc = await this.dataRef.doc(docRef.id).get()
        let result = doc.data()
        result.id = doc.id
        return result
      }
      else {
        return { message: "Record not inserted" }
      }
    }

    catch (e) {
      throw new Error(
        `Firestore DB error ${e.message} `
      );
    }

  }

}

module.exports = OrderDao;