import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <header className="masthead">
          <div className="container h-100">
            <div className="row h-100 align-items-center">
              <div className="col-12 text-center">
                <h1 className="font-weight-light">Run technical screening</h1>
                <p className="lead">Fast. Simple. Secure</p>
              </div>
            </div>
          </div>
        </header>
        <div>
          <p>Some text here</p>
        </div>
      </div>
    );
  }
}
