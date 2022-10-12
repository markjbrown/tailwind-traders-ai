class RecommendedDao {
  constructor(serviceAccount) {
    this.Firebaseclient = serviceAccount;
    this.collectionId = "recommendations";
    this.dataRef = this.Firebaseclient.collection(this.collectionId);
  }

  async init() {
    let snapshot = await this.dataRef.limit(1).get();
    if (snapshot._size == 1) return;

    await this.dataRef.add({ email: "example@example.com", typeid: 1 });
  }

  async findRelated(typeid, email) {
    let typeidToNumber = parseInt(typeid);

    const snapshot = await this.dataRef
      .where("email", "==", email)
      .where("typeid", "==", typeidToNumber)
      .get();
    if (snapshot._size == 0) {
      console.log("No matching documents.");
      return;
    }
    var result = [];
    snapshot.forEach((doc) => {
      let res = doc.data();
      result.push({
        email: res.email,
        typeid: res.typeid,
        recommendations: res.recommendations,
      });
    });
    return result;
  }
}

module.exports = RecommendedDao;
