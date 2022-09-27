import React, { Component, Fragment } from "react";
import { connect } from 'react-redux';
import { LoadingSpinner } from "../../shared/index";
import { NamespacesConsumer } from "react-i18next";

class ThankYou extends Component {
    continueShopping = async () => {
        this.props.history.push('/');
    }

    render() {
        return (
            <NamespacesConsumer>
                {t => (
                    <Fragment>
                        <div className="thank-you__heading my-3">
                            <h2 className="thank-you__title">{t("thankYou.title")}</h2>
                        </div>
                        <div className="thank-you__main">
                            <button
                                className={`btn btn--primary btn--continue my-3`}
                                onClick={this.continueShopping}
                                >
                                {t("thankYou.continue")}
                            </button>
                        </div>
                    </Fragment>

                )}
            </NamespacesConsumer>
        )
    }
}

const mapStateToProps = state => state.login;

export default connect(mapStateToProps)(ThankYou);