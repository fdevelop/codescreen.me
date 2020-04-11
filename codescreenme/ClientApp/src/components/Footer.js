import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Footer extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <footer className="mastfoot mt-auto border-top box-shadow py-3">
        <Container>
          <p><a href="/">CodeScreen.me</a> is an open-source SaaS project. Follow <a href="https://github.com/fdevelop/codescreen.me" target="_blank">GitHub</a> page for more information.</p>
        </Container>
      </footer>
    );
  }
}
