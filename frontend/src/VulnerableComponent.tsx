// TEST FILE — intentionally vulnerable code to trigger SAST findings
// DO NOT use in production

import React, { useState } from "react";

// Vulnerability 1: XSS via dangerouslySetInnerHTML with unsanitized user input
export function XSSComponent() {
  const [userInput, setUserInput] = useState("");

  return (
    <div>
      <input onChange={(e) => setUserInput(e.target.value)} />
      {/* semgrep: react.dangerouslysetinnerhtml */}
      <div dangerouslySetInnerHTML={{ __html: userInput }} />
    </div>
  );
}

// Vulnerability 2: eval() with user-controlled data
export function EvalComponent() {
  const params = new URLSearchParams(window.location.search);
  const expr = params.get("expr") ?? "";

  // semgrep: javascript.lang.security.audit.eval-detected
  const result = eval(expr); // nosem — intentional for SAST test
  return <pre>{String(result)}</pre>;
}

// Vulnerability 3: document.write with user input (DOM XSS)
export function DocumentWriteComponent() {
  const userMsg = new URLSearchParams(window.location.search).get("msg") ?? "";
  // semgrep: javascript.browser.security.dom-xss
  document.write(userMsg);
  return null;
}

// Vulnerability 4: insecure use of localStorage with sensitive key name
export function StoreToken() {
  const token = "super-secret-jwt-value";
  localStorage.setItem("auth_token", token);
  return <span>Token stored</span>;
}
