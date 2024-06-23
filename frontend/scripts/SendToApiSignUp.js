let user;

document
  .getElementById("signupForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const username = document.getElementById("Username").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const repeatPassword = document.getElementById("RepeatPassword").value;

    if (password !== repeatPassword) {
      alert("Passwords do not match!");
      return;
    }

    if (password.length < 8 || repeatPassword.length < 8) {
      alert("password is too short");
      return;
    }

    user = {
      Username: username,
      Email: email,
      Password: password,
    };

    fetch("https://localhost:44358/api/Registration/registration", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(user),
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        return response.text();
      })
      .then((data) => {
        localStorage.setItem("user", data);
        window.location.href = "main.html";
      })
      .catch((error) => {
        console.error("Error:", error);
      });
  });
