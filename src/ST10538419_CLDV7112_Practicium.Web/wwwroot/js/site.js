(() => {
  const toggle = document.querySelector(".nav-toggle");
  const nav = document.querySelector("#site-nav");
  if (!toggle || !nav) return;
  nav.addEventListener("shown.bs.collapse", () => toggle.setAttribute("aria-expanded", "true"));
  nav.addEventListener("hidden.bs.collapse", () => toggle.setAttribute("aria-expanded", "false"));
})();
