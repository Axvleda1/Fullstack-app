document
  .getElementById("signupForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const username = document.getElementById("Username").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const repeatPassword = document.getElementById("RepeatPassword").value;

    if (password !== repeatPassword) {
      displayError("Passwords do not match.");
      return;
    }

    if (password.length < 8 || repeatPassword.length < 8) {
      displayError("Password should be at least 8 characters long.");
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
        if (response.status === 409) {
          throw new Error("User already exists.");
        }
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
        displayError(error.message);
      });
  });

function displayError(message) {
  const errorElement = document.getElementById("errorMessage");
  if (errorElement) {
    errorElement.textContent = message;
    errorElement.style.display = "block";
    errorElement.style.color = "red";
  } else {
    console.error("Error element not found to display error:", message);
  }
}
