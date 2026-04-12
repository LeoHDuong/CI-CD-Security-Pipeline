// TEST FILE — intentionally vulnerable code to trigger SAST findings
// DO NOT use in production

import { useState, useRef, useEffect } from "react";

// Vulnerability 1: XSS — dangerouslySetInnerHTML with unsanitized user input
// Triggers: react.dangerouslysetinnerhtml (p/react)
export function XSSComponent() {
  const [userInput, setUserInput] = useState("");

  return (
    <div>
      <input onChange={(e) => setUserInput(e.target.value)} />
      <div dangerouslySetInnerHTML={{ __html: userInput }} />
    </div>
  );
}

// Vulnerability 2: eval() with user-controlled data from URL
// Triggers: javascript.lang.security.audit.eval-detected (p/javascript)
export function EvalComponent() {
  const params = new URLSearchParams(window.location.search);
  const expr = params.get("expr") ?? "";
  const result = eval(expr);
  return <pre>{String(result)}</pre>;
}

// Vulnerability 3: Direct innerHTML assignment with user data (DOM XSS)
// Triggers: javascript.browser.security.dom-xss.dom-xss (p/javascript)
export function InnerHTMLComponent() {
  const userMsg = new URLSearchParams(window.location.search).get("msg") ?? "";
  const ref = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (ref.current) {
      ref.current.innerHTML = userMsg;
    }
  }, [userMsg]);
  return <div ref={ref} />;
}

// Vulnerability 4: postMessage with wildcard origin
// Triggers: javascript.browser.security.wildcard-postmessage.wildcard-postmessage (p/javascript)
export function PostMessageComponent() {
  const sendData = (data: string) => {
    window.parent.postMessage(data, "*");
  };
  return <button onClick={() => sendData("secret")}>Send</button>;
}
