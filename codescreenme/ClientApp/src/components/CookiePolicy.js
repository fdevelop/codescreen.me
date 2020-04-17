import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { Link } from 'react-router-dom';

export class CookiePolicy extends Component {
  static displayName = CookiePolicy.name;

  render() {
    return (
      <Container className="py-3 custom-page-area">
        <div className="row">
          <div className="col-sm">
            <h2 id="tabelLabel" >Cookie Policy</h2>
          </div>
        </div>
        <p>
          Cookie text
        </p>
        <div className="footerFix"></div>
      </Container>
    );
  }
}
