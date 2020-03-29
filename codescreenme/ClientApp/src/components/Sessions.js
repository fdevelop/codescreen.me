import React, { Component } from 'react';

export class Sessions extends React.Component {
  static displayName = Sessions.name;

  constructor(props) {
    super(props);
    this.state = { sessions: [], loading: true };
    this.populateData = this.populateData.bind(this);
    this.removeSession = this.removeSession.bind(this);
  }

  componentDidMount() {
    this.populateData();
  }

  removeSession(id) {
    let currentComponent = this;

    if (window.confirm('Are you sure you wish to delete this item?')) {
      fetch('api/sessions/' + id, { method: 'DELETE' }).then(function (response) {
        if (response.ok) {
          currentComponent.populateData();
          return;
        }

        window.alert('Internal error happened');
      });
    }
  }

  addSession() {
    let currentComponent = this;

    fetch('api/sessions', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ 'code': '', 'dateCreated': Date.now }) }).then(function (response) {
      if (response.ok) {
        currentComponent.populateData();
        return;
      }

      window.alert('Internal error happened');
    });
  }

  static renderTable(data, selfReference) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Id</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data.map(obj =>
            <tr key={obj.id}>
              <td>{obj.dateCreated}</td>
              <td>{obj.id}</td>
              <td><a className="btn btn-primary btn-sm" href={"/code/" + obj.id}>Join session</a>
                &nbsp;<button className="btn btn-danger btn-sm" onClick={(e) => selfReference.removeSession(obj.id, e)}>Delete session</button></td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Sessions.renderTable(this.state.sessions, this);

    return (
      <div>
        <div className="container">
          <div className="row">
            <div className="col-sm">
              <h2 id="tabelLabel" >Sessions</h2>
            </div>
            <div className="col-sm">
              <button className="btn btn-primary float-right" onClick={(e) => this.addSession(e)}>Create new session</button>
            </div>
          </div>
        </div>
        {contents}
      </div>
    );
  }

  async populateData() {
    const response = await fetch('api/sessions');
    const data = await response.json();
    this.setState({ sessions: data, loading: false });
  }
}
