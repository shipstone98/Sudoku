// swift-tools-version: 6.3

import PackageDescription

let package = Package(
    name: "Sudoku",
    products: [.library(name: "Sudoku", targets: ["Sudoku"])],
    dependencies: [.package(name: "Utilities", path: "../utilities")],
    targets: [
        .target(
            name: "Sudoku",
            dependencies: [.product(name: "Utilities", package: "Utilities")]
        ),
        .testTarget(name: "SudokuTests", dependencies: ["Sudoku"])
    ],
    swiftLanguageModes: [.v6]
)
