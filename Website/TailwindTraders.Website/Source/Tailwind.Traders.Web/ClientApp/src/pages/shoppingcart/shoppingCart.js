import React, { Component, Fragment } from "react";
import { connect } from 'react-redux';

import { LoadingSpinner } from "../../shared/index";
import { NamespacesConsumer } from "react-i18next";

import ShoppingCartCard from "./shoppingCartCard";
import { CartService } from '../../services';

class ShoppingCart extends Component {
    constructor(props) {        
        super(props);
        this.state = {
            shoppingCart: [],
            loading: true,
            quantity: null,
            isPulling: true
        }
        this.updateQty = this.updateQty.bind(this);
        this.assignShoppingCartInterval = null;
        this.email = null;
    }

    async componentDidMount() {
        this.setState({ quantity: this.props.quantity });

        if (this.state.isPulling) {
            this.assignShoppingCartInterval = await this.assignShoppingCart();
        }
    }

    componentWillUnmount() {
        if (this.assignShoppingCartInterval) {
            clearInterval(this.assignShoppingCartInterval);
        }
    }

    async assignShoppingCart() {
        return setInterval(async () => {
            const shoppingCart = await CartService.getShoppingCart(this.props.userInfo.token);
            this.setState({ shoppingCart, loading: false });
            if (this.assignShoppingCartInterval) {
                clearInterval(this.assignShoppingCartInterval);
            }
        }, 1000);
    }

    async updateQty(id, qty) {
        this.setState({ isPulling: true }, async () => {
            if (qty > 0) {
                await CartService.updateQuantity(id, qty, this.props.userInfo.token)
                await this.updateShoppingCartState(id, qty);
            } else {
                await CartService.deleteProduct(id, this.props.userInfo.token);
                await this.cleanShoppingCartState(id, qty);
            }
            this.setState({ isPulling: false })

            await this.setQuantityState();

            this.props.ShoppingCart(this.state.quantity)
        });
    }

    async cleanShoppingCartState(id, qty) {
        this.state.shoppingCart.map((item) => {
            if (item._cdbid === id) {
                item.qty = qty;
                const index = this.state.shoppingCart.indexOf(item);
                if (index > -1) {
                    this.state.shoppingCart.splice(index, 1);
                }
            }
            this.setState({ shoppingCart: this.state.shoppingCart });
            return this.state.shoppingCart;
        });
    }

    async updateShoppingCartState(id, qty) {
        const shoppingCart = this.state.shoppingCart.map((item) => {
            if (item._cdbid === id) {
                item.qty = qty;
            }
            return item;
        });
        this.setState({ shoppingCart });
    }

    checkout = async () => {
        await CartService.checkout(this.props.userInfo.user.email, this.props.userInfo.token);
        this.props.history.push('/thank-you');
    }

    async setQuantityState() {
        const quantity = this.state.shoppingCart.reduce((oldQty, { qty }) => oldQty + qty, 0);
        this.setState({ quantity })
    }

    render() {
        const { shoppingCart, loading, isPulling } = this.state
        return (
            <NamespacesConsumer>
                {t => (
                    <Fragment>
                        {loading && isPulling ? <LoadingSpinner /> : <Fragment>
                            <h2 className="shopping-card__title grid__heading">{t("shoppingCart.title")}</h2>
                            <div className="shopping-card__grid">
                                {shoppingCart && shoppingCart.map((shoppingCart, index) => (
                                    <ShoppingCartCard {...shoppingCart} updateQty={this.updateQty} key={index} />
                                ))}
                            </div>
                            <div className="shopping__checkout">
                                <button
                                    className={`btn btn--primary btn--checkout my-3`}
                                    onClick={this.checkout}
                                    >
                                    {t("shoppingCart.checkout")}
                                </button>
                            </div>
                        </Fragment>}
                    </Fragment>
                )}
            </NamespacesConsumer>
        );
    }
}

const mapStateToProps = state => state.login;

export default connect(mapStateToProps)(ShoppingCart);