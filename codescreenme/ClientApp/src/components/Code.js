import React, { Component } from 'react';
import { UnControlled as CodeMirror } from 'react-codemirror2'

require('codemirror/lib/codemirror.css');
require('codemirror/theme/material.css');
require('codemirror/theme/neat.css');

require('codemirror/mode/htmlmixed/htmlmixed');
require('codemirror/mode/javascript/javascript');
require('codemirror/mode/clike/clike');
require('codemirror/mode/python/python');
require('codemirror/mode/powershell/powershell');
require('codemirror/mode/shell/shell');
require('codemirror/mode/sql/sql');

export class Code extends React.Component {
  static displayName = Code.name;

  constructor(props) {
    super(props);
    const idFromUrlIndex = props.location.pathname.lastIndexOf('/') + 1;
    const idFromUrl = props.location.pathname.slice(idFromUrlIndex);
    this.state = { id: idFromUrl, loading: true };
    this.editorInstance = null;
    this.populateData = this.populateData.bind(this);
    this.editorTick = this.editorTick.bind(this);
    this.editorModeChange = this.editorModeChange.bind(this);
  }

  componentDidMount() {
    this.populateData();
  }

  render() {
    if (!this.state.stateObject) {
      return (
        <div>
          Nothing to render...
        </div>
        )
    }

    return (
      <div>
        <div>
          <label htmlFor="selectEditorMode">Syntax language:</label>
          <select id="selectEditorMode" value={this.state.editorMode} onChange={this.editorModeChange}>
            <option value="clike">C++/C#/Java</option>
            <option value="python">Python</option>
            <option value="javascript">JavaScript</option>
            <option value="shell">Shell</option>
            <option value="powershell">PowerShell</option>
            <option value="sql">SQL</option>
            <option value="htmlmixed">HTML</option>
          </select>
        </div>

        <div>
          <span>Owner:</span>
          <span className="badge badge-primary">{this.state.stateObject.codeSession.owner}</span>
        </div>
        <div>
          <span>Participants:</span>
          {this.state.stateObject.codeSession.participants ? this.state.stateObject.codeSession.participants.map((p) => p === this.state.stateObject.codeSession.owner ? <span key={p} className="badge badge-primary">{p}</span> : <span key={p} className="badge badge-secondary">{p}</span>) : <span>Unavailable</span>}
        </div>

        <CodeMirror
          value={this.state.stateObject.codeSession.code}
          options={{
            lineNumbers: true,
            mode: this.state.editorMode,
            readOnly: this.state.stateObject.role === 1
          }}
          editorDidMount={editor => { this.editorInstance = editor }}
          onChange={(editor, data, value) => {
            const response = fetch('api/code/' + this.state.id, {
              method: 'PUT',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ 'text': value })
            });

            let newStateObject = this.state.stateObject;
            newStateObject.codeSession.code = value;

            this.setState({ stateObject: newStateObject });
          }}
        />
      </div>
    );
  }

  editorModeChange(event) {
    this.setState({ editorMode: event.target.value });
  }

  editorTick() {
    this.populateData();
  }

  async populateData() {
    const response = await fetch('api/code/' + this.state.id);
    const data = await response.json();

    if (data) {
      this.setState({ id: data.codeSession.id, stateObject: data, loading: false });
    }

    if (this.state.stateObject.role === 1) {
      setTimeout(this.editorTick, 1000);
    }
  }
}
