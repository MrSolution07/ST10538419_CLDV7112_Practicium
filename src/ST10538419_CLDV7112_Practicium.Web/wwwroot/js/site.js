(() => {
  const toggle = document.querySelector(".nav-toggle");
  const nav = document.querySelector("#site-nav");
  if (toggle && nav) {
    nav.addEventListener("shown.bs.collapse", () => toggle.setAttribute("aria-expanded", "true"));
    nav.addEventListener("hidden.bs.collapse", () => toggle.setAttribute("aria-expanded", "false"));
  }

  document.querySelectorAll("[data-fill-user]").forEach((button) => {
    button.addEventListener("click", () => {
      const username = document.getElementById("Username");
      const password = document.getElementById("Password");
      if (username) username.value = button.getAttribute("data-fill-user") || "";
      if (password) password.value = button.getAttribute("data-fill-password") || "";
      if (username) username.focus();
    });
  });

  const summary = document.querySelector(".validation-summary-errors");
  if (summary) {
    const target = document.getElementById("form-errors") || summary;
    target.setAttribute("tabindex", "-1");
    target.focus();
  }

  document.querySelectorAll("form").forEach((form) => {
    form.addEventListener("submit", () => {
      const button = form.querySelector("[type=submit]");
      if (!button || button.dataset.busy === "off") return;
      window.setTimeout(() => {
        button.setAttribute("aria-busy", "true");
        button.textContent = "Working...";
      }, 0);
    });
  });
})();
