﻿import React, { Component } from 'react';
import { Controlled as CodeMirror } from 'react-codemirror2'
import * as signalR from "@microsoft/signalr";

require('codemirror/lib/codemirror.css');
require('codemirror/theme/material.css');
require('codemirror/theme/neat.css');

require('codemirror/mode/htmlmixed/htmlmixed.js');
require('codemirror/mode/javascript/javascript.js');
require('codemirror/mode/clike/clike.js');
require('codemirror/mode/python/python.js');
require('codemirror/mode/powershell/powershell.js');
require('codemirror/mode/shell/shell.js');
require('codemirror/mode/sql/sql.js');

export class Code extends React.Component {
  static displayName = Code.name;

  constructor(props) {
    super(props);

    const idFromUrlIndex = props.location.pathname.lastIndexOf('/') + 1;
    const idFromUrl = props.location.pathname.slice(idFromUrlIndex);

    this.state = {
      id: idFromUrl,
      codeConnection: null,
      editorMode: 'text/x-csharp',
      loading: true,
      hubConnection: null
    };
    this.editorInstance = null;

    this.populateData = this.populateData.bind(this);
    this.editorModeChange = this.editorModeChange.bind(this);

    this.highlightTextAction = this.highlightTextAction.bind(this);
    this.highlightEraseAction = this.highlightEraseAction.bind(this);
    this.highlightEraseClick = this.highlightEraseClick.bind(this);
    this.highlightSelectionClick = this.highlightSelectionClick.bind(this);
    
    this.receiveCodeUpdateListener = this.receiveCodeUpdateListener.bind(this);
    this.setCodeHighlightListener = this.setCodeHighlightListener.bind(this);
    this.removeCodeHighlightsListener = this.removeCodeHighlightsListener.bind(this);
    this.sendCodeUpdate = this.sendCodeUpdate.bind(this);
    this.participantListener = this.participantListener.bind(this);
    this.participantJoined = this.participantJoined.bind(this);

    this.setupSignalR = this.setupSignalR.bind(this);
  }

  componentDidMount() {
    this.populateData();

    this.setupSignalR();
  }

  setupSignalR() {
    if (this.state.hubConnection !== null) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/codehub")
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.setState({ hubConnection: connection }, () => {
      this.state.hubConnection
        .start()
        .then((v) => this.participantJoined())
        .catch(err => console.error('Error while establishing connection with codehub, application may not work as expected. ' + err));

      this.state.hubConnection
        .on("ReceiveCodeUpdate", this.receiveCodeUpdateListener);

      this.state.hubConnection
        .on("RemoveCodeHighlights", this.removeCodeHighlightsListener);

      this.state.hubConnection
        .on("SetCodeHighlight", this.setCodeHighlightListener);

      this.state.hubConnection
        .on("ParticipantJoined", this.participantListener);
    });
  }

  receiveCodeUpdateListener(user, sessionId, code) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.role === 0) {
      return;
    }

    let newCodeConnection = this.state.codeConnection;
    newCodeConnection.codeSession.code = code;

    this.setState({ codeConnection: newCodeConnection });
  }

  removeCodeHighlightsListener(user, sessionId) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.role === 0) {
      return;
    }

    this.highlightEraseAction(this.editorInstance);
  }

  setCodeHighlightListener(user, sessionId, codeCursor) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.role === 0) {
      return;
    }

    this.highlightTextAction(this.editorInstance, codeCursor.highlightFrom, codeCursor.highlightTo);
  }

  participantListener(user, sessionId) {
    if (this.state.codeConnection.codeSession.id == sessionId && !this.state.codeConnection.codeSession.participants.includes(user)) {
      let newCodeConnection = this.state.codeConnection;
      newCodeConnection.codeSession.participants.push(user);

      this.setState({ codeConnection: newCodeConnection });
    }
  }

  highlightTextAction(editor, from, to) {
    const finalObject = {
      'highlightFrom': { line: from.line, ch: from.ch },
      'highlightTo': { line: to.line, ch: to.ch }
    };

    editor.markText(
      finalObject.highlightFrom,
      finalObject.highlightTo,
      { css: 'background-color: #E6E6FA' }
    );

    return finalObject;
  }

  highlightSelectionClick(eventData, editor) {
    if (!editor.somethingSelected() || this.state.codeConnection.role === 1) {
      return;
    }

    const selFrom = editor.getCursor('from');
    const selTo = editor.getCursor('to');

    const finalObject = this.highlightTextAction(editor, selFrom, selTo);

    editor.focus();

    // Server side

    let newCodeConnection = this.state.codeConnection;
    newCodeConnection.codeSession.codeHighlights.push(finalObject);

    this.state.hubConnection.invoke("SetCodeHighlight", this.state.codeConnection.user, this.state.id, finalObject)
      .then(function (r) { return true; })
      .catch(function (err) {
        console.error(err.toString());
        return false;
      });

    this.setState({ codeConnection: newCodeConnection });
  }

  highlightEraseAction(editor) {
    let marks = editor.getAllMarks();
    for (const m of marks) {
      m.clear();
    }
  }

  highlightEraseClick(eventData, editor) {
    if (this.state.codeConnection.role === 1) {
      return;
    }

    this.highlightEraseAction(editor);

    editor.focus();

    // Server side

    let newCodeConnection = this.state.codeConnection;
    newCodeConnection.codeSession.codeHighlights.length = 0;

    this.state.hubConnection.invoke("RemoveCodeHighlights", this.state.codeConnection.user, this.state.id)
      .then(function (r) { return true; })
      .catch(function (err) {
        console.error(err.toString());
        return false;
      });

    this.setState({ codeConnection: newCodeConnection });
  }

  sendCodeUpdate(newCodeValue) {
    this.state.hubConnection.invoke("ReceiveCodeUpdate", this.state.codeConnection.user, this.state.id, newCodeValue)
      .then(function (r) { return true; })
      .catch(function (err) {
        console.error(err.toString());
        return false;
      });
  }

  participantJoined() {
    this.state.hubConnection.invoke("ParticipantJoined", this.state.codeConnection.user, this.state.id)
      .then(function (r) { return true; })
      .catch(function (err) {
        console.error(err.toString());
        return false;
      });
  }

  render() {
    if (!this.state.codeConnection) {
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
            <option value="text/x-csharp">C#</option>
            <option value="text/x-java">Java</option>
            <option value="text/x-c++src">C/C++</option>
            <option value="python">Python</option>
            <option value="javascript">JavaScript</option>
            <option value="shell">Shell</option>
            <option value="powershell">PowerShell</option>
            <option value="sql">SQL</option>
            <option value="htmlmixed">HTML</option>
          </select>
        </div>

        <div>
          <span>Participants:</span>
          {this.state.codeConnection.codeSession.participants
            ? this.state.codeConnection.codeSession.participants.map((p) =>
              p === this.state.codeConnection.codeSession.owner
                ? <span key={p} className="badge badge-secondary">{p} (Owner)</span>
                : <span key={p} className="badge badge-secondary">{p}</span>)
            : <span>Unavailable</span>}
        </div>

        <div>
          <span>Actions:</span>
          <button className="btn btn-primary" onClick={(e) => { this.highlightSelectionClick(e, this.editorInstance) }}>Highlight selection</button>
          <button className="btn btn-danger" onClick={(e) => { this.highlightEraseClick(e, this.editorInstance) }}>Erase highlight</button>
        </div>

        <CodeMirror
          value={this.state.codeConnection.codeSession.code}
          options={{
            autofocus: true,
            lineNumbers: true,
            mode: this.state.editorMode,
            readOnly: this.state.codeConnection.role === 1
          }}
          editorDidMount={(editor) => {
            this.editorInstance = editor
          }}
          onBeforeChange={(editor, data, value) => {
            let newCodeConnection = this.state.codeConnection;
            newCodeConnection.codeSession.code = value;

            this.sendCodeUpdate(value);

            this.setState({ codeConnection: newCodeConnection });
          }}
          onChange={(editor, data, value) => {
          }}
        />
      </div>
    );
  }

  editorModeChange(event) {
    this.setState({ editorMode: event.target.value });
  }

  async populateData() {
    const response = await fetch('api/code/' + this.state.id);
    const data = await response.json();

    if (data) {
      this.setState({ id: data.codeSession.id, codeConnection: data, loading: false });

      this.highlightEraseAction(this.editorInstance);
      for (const cc of this.state.codeConnection.codeSession.codeHighlights) {
        this.highlightTextAction(this.editorInstance, cc.highlightFrom, cc.highlightTo);
      }
    }
  }
}
