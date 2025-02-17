const quizData = [
    {
        question: "What does HTML stand for?",
        a: "Hyper Text Markup Language",
        b: "Hot Mail",
        c: "How To Make Lasagna",
        d: "None of the above",
        correct: "a"
    },
    {
        question: "Which language runs in a web browser?",
        a: "Java",
        b: "C",
        c: "Python",
        d: "JavaScript",
        correct: "d"
    },
    {
        question: "What year was JavaScript created?",
        a: "1995",
        b: "1994",
        c: "2000",
        d: "None of the above",
        correct: "a"
    },
    {
        question: "Which tag is used to define an image in HTML?",
        a: "<img>",
        b: "<image>",
        c: "<src>",
        d: "<pic>",
        correct: "a"
    },
    {
        question: "What is the purpose of CSS?",
        a: "To style pages",
        b: "To add functionality",
        c: "To structure content",
        d: "To connect to databases",
        correct: "a"
    }
];
let questionIndex = 0;
let scoredQuestions = new Set();
let question = document.getElementById('question');
let quizButtons = document.querySelectorAll('.btn');
let nextButton = document.getElementById('next');
let currentQuizData = quizData[questionIndex];
function loadQuestion() {
    currentQuizData = quizData[questionIndex];
    question.innerText = currentQuizData.question;
    let option='a';
    quizButtons.forEach((button) => {
        button.innerText = currentQuizData[option];
        option=nextChar(option);
        button.style.backgroundColor="white";
    });
    nextButton.innerText="Next";
}
function nextChar(c) {
    return String.fromCharCode(c.charCodeAt(0) + 1);
}
function checkAns(ind){
    let correctAns=quizData[questionIndex].correct;
    if(ind==correctAns){
        document.getElementById(ind).style.backgroundColor="green";
        scoredQuestions.add(ind);
    }
    else{
        document.getElementById(correctAns).style.backgroundColor="green";
        document.getElementById(ind).style.backgroundColor="red";
    }
}
function nextQuestion() {
    if (questionIndex < quizData.length - 1) {
        questionIndex++;
        loadQuestion();
    } else {
        showFinalScore();
    }
}
function showFinalScore() {
    document.getElementById("quiz-frame").innerHTML = `<div class="fs-5"><h2>You answered correctly at ${scoredQuestions.size}/${quizData.length} questions.</h2></div>`;
}

loadQuestion();