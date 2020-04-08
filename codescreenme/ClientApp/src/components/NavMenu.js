import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.getUserName = this.getUserName.bind(this);
    this.changeUserName = this.changeUserName.bind(this);
    this.getUserDisplayNameFromCookie = this.getUserDisplayNameFromCookie.bind(this);

    this.state = {
      collapsed: true,
      userDisplayName: this.getUserDisplayNameFromCookie()
    };
  }

  getUserDisplayNameFromCookie() {
    let cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UniqueUserDisplayName\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    if (!cookieValue) {
      cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UniqueUser\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    }

    return cookieValue;
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  changeUserName() {
    let cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)UniqueUser\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    let prom = window.prompt('Enter new display name for your user:', this.getUserName());

    if (prom != null) {
      let finalObject = { id: cookieValue, displayName: prom };

      const response = fetch('api/users/' + cookieValue, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(finalObject)
      });

      this.setState({
        userDisplayName: prom
      });

      return true;
    }
  }

  getUserName() {
    return this.state.userDisplayName;
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">CodeScreen.me</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/sessions">Dashboard</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/about">About</NavLink>
                </NavItem>
                <NavItem>
                  <div className="dropdown show">
                    <a className="btn btn-info dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">{this.getUserName()}</a>

                    <div className="dropdown-menu" aria-labelledby="dropdownMenuLink">
                      <a className="dropdown-item" href="#" onClick={(e) => this.changeUserName()}>Change user name</a>
                    </div>
                  </div>
                </NavItem>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
