import React, { Component } from 'react';
import { Container } from 'reactstrap';
import CookieConsent from "react-cookie-consent";
import { NavMenu } from './NavMenu';
import { Footer } from './Footer';
import { Link } from 'react-router-dom';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <>
        <NavMenu />
        {this.props.children}
        <CookieConsent
          buttonText="Acknowledge">
          This website uses cookies according to the <Link to="/articles/cookie">cookie policy</Link>.By proceeding further, you are accepting our <Link to="/articles/cookie">cookie policy</Link>.
        </CookieConsent>
        <Footer />
      </>
    );
  }
}
