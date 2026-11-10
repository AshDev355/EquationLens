(function () {
  // Show / hide password
  document.querySelectorAll('[data-toggle-password]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var input = document.querySelector(btn.getAttribute('data-toggle-password'));
      var show = input.type === 'password';
      input.type = show ? 'text' : 'password';
      btn.textContent = show ? 'Hide' : 'Show';
    });
  });

  // Static login: send each role to its own home page
  var login = document.getElementById('loginForm');
  if (login) {
    login.addEventListener('submit', function (e) {
      e.preventDefault();
      if (!login.checkValidity()) { login.classList.add('was-validated'); return; }
      var role = document.getElementById('role').value;
      window.location.href = role === 'admin' ? 'admin-home.html' : 'user-home.html';
    });
  }

  // Static signup: check passwords match, then go to login
  var signup = document.getElementById('signupForm');
  if (signup) {
    signup.addEventListener('submit', function (e) {
      e.preventDefault();
      var pw = document.getElementById('password'), cf = document.getElementById('confirm');
      cf.setCustomValidity(pw.value === cf.value ? '' : 'Passwords do not match');
      if (!signup.checkValidity()) { signup.classList.add('was-validated'); return; }
      window.location.href = 'login.html';
    });
  }

  // User dashboard: preview the chosen image and show the demo result
  var file = document.getElementById('fileInput');
  if (file) {
    var preview = document.getElementById('previewImg');
    var drop = document.getElementById('dropzone');
    file.addEventListener('change', function () {
      if (file.files && file.files[0]) {
        preview.src = URL.createObjectURL(file.files[0]);
      }
    });
    drop.addEventListener('dragover', function (e) { e.preventDefault(); });
    drop.addEventListener('drop', function (e) {
      e.preventDefault();
      if (e.dataTransfer.files[0]) { preview.src = URL.createObjectURL(e.dataTransfer.files[0]); }
    });
    var solve = document.getElementById('solveBtn');
    solve.addEventListener('click', function () {
      var steps = solveEquation(document.getElementById('latex').value);
      var list = document.querySelector('#solutionSteps .step-list');
      list.innerHTML = steps.map(function (t, i) {
        var last = i === steps.length - 1;
        return '<li>(' + (i + 1) + ') &nbsp;' + (last ? '<span class="answer">' + t + '</span>' : t) + '</li>';
      }).join('');
      document.getElementById('solutionEmpty').classList.add('d-none');
      document.getElementById('solutionSteps').classList.remove('d-none');
    });

    // Clear history
    var clear = document.getElementById('clearHistory');
    if (clear) {
      clear.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelectorAll('.history-item').forEach(function (n) { n.remove(); });
        clear.textContent = 'History cleared';
      });
    }
  }

  // ---- Linear / quadratic solver (runs in the browser) ----
  function fmt(n) {
    if (Number.isInteger(n)) return String(n).replace('-', '&minus;');
    for (var d = 2; d <= 100; d++) {
      var num = n * d;
      if (Math.abs(num - Math.round(num)) < 1e-9) return (Math.round(num) + '/' + d).replace('-', '&minus;');
    }
    return n.toFixed(4).replace(/0+$/, '').replace('-', '&minus;');
  }
  function parseSide(str, sign, co) {
    (str.match(/[+-]?[^+-]+/g) || []).forEach(function (t) {
      var m = t.match(/^([+-]?)(\d*\.?\d*)(x(\^\{?2\}?)?)?$/);
      if (!m) throw new Error('bad');
      var v = m[2] === '' ? (m[3] ? 1 : NaN) : parseFloat(m[2]);
      if (isNaN(v)) throw new Error('bad');
      v *= (m[1] === '-' ? -1 : 1) * sign;
      if (m[4]) co.a += v; else if (m[3]) co.b += v; else co.c += v;
    });
  }
  function solveEquation(src) {
    var s = src.replace(/\\cdot|\\times|\s|\{\}/g, '').replace(/\u2212/g, '-').replace(/\*/g, '');
    var parts = s.split('=');
    var co = { a: 0, b: 0, c: 0 };
    try {
      if (parts.length !== 2) throw new Error('bad');
      parseSide(parts[0], 1, co);
      parseSide(parts[1], -1, co);
    } catch (e) {
      return ['Sorry, I could not read that. Use a form like 2x^2 - 5x + 3 = 0 or 3x + 7 = 22.'];
    }
    var a = co.a, b = co.b, c = co.c, out = [];
    if (a === 0 && b === 0) return [c === 0 ? 'Every x works (identity).' : 'No solution.'];
    if (a === 0) {
      out.push('Rearrange: ' + fmt(b) + 'x ' + (c < 0 ? '&minus; ' + fmt(-c) : '+ ' + fmt(c)) + ' = 0');
      out.push(fmt(b) + 'x = ' + fmt(-c));
      out.push('x = ' + fmt(-c / b));
      return out;
    }
    var D = b * b - 4 * a * c;
    out.push('a = ' + fmt(a) + ', b = ' + fmt(b) + ', c = ' + fmt(c));
    out.push('&Delta; = b&sup2; &minus; 4ac = ' + fmt(D));
    if (D < 0) { out.push('&Delta; &lt; 0, so there are no real roots.'); return out; }
    if (D === 0) {
      out.push('x = &minus;b / 2a = ' + fmt(-b / (2 * a)));
      out.push('x = ' + fmt(-b / (2 * a)));
      return out;
    }
    var r = Math.sqrt(D), sq = Number.isInteger(r) ? fmt(r) : '&radic;' + fmt(D);
    out.push('x = (' + fmt(-b) + ' &plusmn; ' + sq + ') / ' + fmt(2 * a));
    var x1 = (-b + r) / (2 * a), x2 = (-b - r) / (2 * a);
    out.push('x = ' + fmt(Math.min(x1, x2)) + ' or x = ' + fmt(Math.max(x1, x2)));
    return out;
  }

  document.querySelectorAll('[data-year]').forEach(function (n) { n.textContent = new Date().getFullYear(); });
})();
