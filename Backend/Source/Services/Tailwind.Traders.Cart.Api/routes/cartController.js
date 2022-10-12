class CartController {
    constructor(shoppingCartDao, recommendedDao, orderDao) {
        this.shoppingCartDao = shoppingCartDao;
        this.recommendedDao = recommendedDao;
        this.orderDao = orderDao
    }

    retrieveUser(req) {
        return req.headers["x-tt-name"];
    }

    async addProduct(req, res) {
        const item = req.body;
        const doc = await this.shoppingCartDao.addItem(item);
        res.status(201).send({ message: `${doc.detailProduct.name} added to shopping cart`, id: doc.id });
    }

    async getProductsByUser(req, res) {

        const user = this.retrieveUser(req);
        const items = await this.shoppingCartDao.find(user);
        res.json(items);
    }

    async updateProductQuantity(req, res) {
        const data = req.body;
        if (!data.qty || !data.id) {
            res.status(400).send({ message: "'id' and/or 'qty' missing" });
        }
        else {
            await this.shoppingCartDao.updateQuantity(data.id, data.qty);
            res.status(201).send({ message: "Product qty updated" });
        }
    }

    async deleteItem(req, res) {
        const data = req.body;
        if (!data.id) {
            res.status(400).send({ message: "'id' missing" });
        }
        else {
            await this.shoppingCartDao.deleteItem(data.id);
            res.status(200).send({ message: "Product deleted" });
        }

    }

    async getRelatedProducts(req, res) {
        const user = this.retrieveUser(req);

        const typeid = req.query.type;
        if (!typeid && !user) {
            res.status(400).send({ message: "'user' or 'productType' missing" });
        } else {
            const items = await this.recommendedDao.findRelated(typeid, user);
            res.json(items);
        }
    }

    async checkout(req, res) {
        const data = req.body;
        const items = await this.shoppingCartDao.find(data.email);
        if (items.length > 0) {
            const order = await this.orderDao.createOrder(data.email, items);
            for (const item of items) {
                await this.shoppingCartDao.deleteItem(item._cdbid);
            }
            res.status(200).send({ message: `checkout for ${data.email} successful`, order: order });
        }
        else {
            res.status(200).send({ message: "No items in cart", order: [] })
        }
    }

    async getPopulerProducts(req, res) {
        console.log("popular product")
        const items = await this.orderDao.getPopularProducts();
        res.json(items);
    }

}

module.exports = CartController;