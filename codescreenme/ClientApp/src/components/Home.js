import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <main className="masthead">
        <Container>
          <div className="row h-100 align-items-center">
            <div className="col-12 text-center">
              <h1 className="font-weight-light">Run technical screening</h1>
              <p className="lead">Fast. Simple. Secure</p>
            </div>
          </div>
        </Container>
      </main>
    );
  }
}
