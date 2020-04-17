import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';

import { Code } from './components/Code';
import { Sessions } from './components/Sessions';

import './custom.css'
import { CookiePolicy } from './components/CookiePolicy';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/sessions' component={Sessions} />
        <Route path='/code' component={Code} />
        <Route path='/articles/cookie' component={CookiePolicy} />
      </Layout>
    );
  }
}
