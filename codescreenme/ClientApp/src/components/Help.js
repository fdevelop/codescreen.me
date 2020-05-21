import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Help extends Component {
  static displayName = Help.name;

  constructor(props) {
    super(props);
  }

  render() {
    return (
      <Container className="py-3 custom-page-area">
        <div className="row mb-3">
          <div className="col-sm">
            <h2 id="tabelLabel" >Quick help</h2>
          </div>
        </div>
        <div className="row mb-2">
          <div className="col-sm">
            <h4>Code sessions</h4>
            <p>Each code session can be shared with unlimited number of people. However, currently it's only one user can be owner and only one user can be Editor at any moment of time. The owner can change editor as many times as necessary. The owner cannot be changed.</p>
            <p>Each session remains active and available for 24 hours since its creation - afterwards, session is archived (removed).</p>
          </div>
        </div>
        <div className="row mb-2">
          <div className="col-sm">
            <h4>Authentication</h4>
            <p>There is no authentication on the site. Each user is identified by the lifetime cookie generated during the first visit to the website. If your browser blocks cookies, website will not function correctly.</p>
          </div>
        </div>
        <div className="row mb-2">
          <div className="col-sm">
            <h4>UI how-to</h4>
            <img src="/help.png" class="img-fluid" alt="help instrctions with visual elements explained" />
          </div>
        </div>
        <div className="row mb-2">
          <div className="col-sm">
            <h4>Feedback</h4>
            <p>Any issues or suggestions can be reported via <a href="https://github.com/fdevelop/codescreen.me" target="_blank" rel="noopener noreferrer">GitHub project page</a>.</p>
          </div>
        </div>
        <div className="footerFix"></div>
      </Container>
    );
  }
}
