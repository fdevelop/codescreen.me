import React, { Component } from 'react';
import { Controlled as CodeMirror } from 'react-codemirror2'
import * as signalR from "@microsoft/signalr";
import { Container } from 'reactstrap';

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
      userSwitcherFlag: false,
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
    this.setUserInControlListener = this.setUserInControlListener.bind(this);
    this.switchUserInControlClick = this.switchUserInControlClick.bind(this);
    this.selectUserInControlClick = this.selectUserInControlClick.bind(this);

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

      this.state.hubConnection
        .on("SetUserInControl", this.setUserInControlListener);
    });
  }

  receiveCodeUpdateListener(user, sessionId, code) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.rights.canEdit) {
      return;
    }

    let newCodeConnection = this.state.codeConnection;
    newCodeConnection.codeSession.code = code;

    this.setState({ codeConnection: newCodeConnection });
  }

  removeCodeHighlightsListener(user, sessionId) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.rights.canEdit) {
      return;
    }

    this.highlightEraseAction(this.editorInstance);
  }

  setCodeHighlightListener(user, sessionId, codeCursor) {
    if (this.state.id !== sessionId || this.state.codeConnection.user === user || this.state.codeConnection.rights.canEdit) {
      return;
    }

    this.highlightTextAction(this.editorInstance, codeCursor.highlightFrom, codeCursor.highlightTo);
  }

  participantListener(user, sessionId) {
    if (this.state.id === sessionId && !this.state.codeConnection.codeSession.participants.includes(user)) {
      let newCodeConnection = this.state.codeConnection;
      newCodeConnection.codeSession.participants.push(user);

      this.setState({ codeConnection: newCodeConnection });
    }
  }

  setUserInControlListener(user, sessionId, newUser) {
    if (this.state.id === sessionId) {
      console.log('I hear u + ' + newUser);
      this.populateData();
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
    if (!editor.somethingSelected() || !this.state.codeConnection.rights.canEdit) {
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
    if (!this.state.codeConnection.rights.canEdit) {
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

  switchUserInControlClick(eventData) {
    if (!this.state.codeConnection.rights.canAdministrate) {
      return;
    }

    this.setState({
      userSwitcherFlag: true
    });
  }

  selectUserInControlClick(newUserInControl) {
    if (!this.state.userSwitcherFlag) {
      return;
    }

    this.state.hubConnection.invoke("SetUserInControl", this.state.codeConnection.user, this.state.id, newUserInControl)
      .then((r) => {
        this.setState({
          userSwitcherFlag: false
        });
      })
      .then((r) => { return true; })
      .catch(function (err) {
        console.error(err.toString());
        return false;
      });
  }

  render() {
    if (!this.state.codeConnection) {
      return (
        <div>
          Code session {this.state.id} does not exist or has been removed.
        </div>
      )
    }

    return (
      <Container className="py-3">
        <div className="code-toolbox">
          <span className="caption">Session</span>
          <span>{this.state.id}</span>
          <span className="float-right">
            <a href={window.location.href} target="_blank">{window.location.href}</a>
          </span>
        </div>

        <div className="code-toolbox">
          <span className="caption">Participants</span>
          {this.state.codeConnection.codeSession.participants
            ? this.state.codeConnection.codeSession.participants.map((p) =>
              <span key={p} className="badge badge-secondary" onClick={() => this.selectUserInControlClick(p)}>
                {p}
                {this.state.codeConnection.codeSession.owner === p ? <>&nbsp;(Owner)</> : <></>}
                {this.state.codeConnection.codeSession.userInControl === p ? <>&nbsp;(Editor)</> : <></>}
                {this.state.codeConnection.codeSession.userInControl !== p ? <>&nbsp;(Spectator)</> : <></>}
                {this.state.codeConnection.user === p ? <>&nbsp;(You)</> : <></>}
              </span>
            )
            : <span>Unavailable</span>}
          <span className="float-right">
            <button disabled={!this.state.codeConnection.rights.canAdministrate} className="btn btn-info btn-sm" onClick={(e) => { this.switchUserInControlClick(e) }}>
              {this.state.userSwitcherFlag ? <>[click on user name/id to choose]</> : <>Give control</>}
            </button>
          </span>
        </div>

        <div className="code-toolbox">
          <span className="caption">Toolbox</span>
          <label className="shift" htmlFor="selectEditorMode">Syntax language</label>
          <select disabled={!this.state.codeConnection.rights.canEdit} className="shift" id="selectEditorMode" value={this.state.editorMode} onChange={this.editorModeChange}>
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
          <label className="shift" htmlFor="highlightButtons">Highlight to all</label>
          <span className="shift" id="highlightButtons">
            <button disabled={!this.state.codeConnection.rights.canEdit} className="btn btn-primary btn-sm" onClick={(e) => { this.highlightSelectionClick(e, this.editorInstance) }}>Highlight selection</button>
            <button disabled={!this.state.codeConnection.rights.canEdit} className="btn btn-danger btn-sm" onClick={(e) => { this.highlightEraseClick(e, this.editorInstance) }}>Erase highlight</button>
          </span>
        </div>

        <div className="codeArea">
        <CodeMirror
          className="h-100"
          value={this.state.codeConnection.codeSession.code}
          options={{
            autofocus: true,
            lineNumbers: true,
            mode: this.state.editorMode,
            readOnly: !this.state.codeConnection.rights.canEdit
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
      </Container>
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
